#!/bin/bash
# Git 자동 업데이트 스크립트
# 60초마다 실행되어 원격 저장소의 변경사항을 확인하고,
# 변경이 있으면 pull 후 서버를 재시작합니다.

REPO_DIR="/Users/d9/Documents/syncthing-d9/github/WebDevelop"
LOG_FILE="/Users/d9/Documents/syncthing-d9/github/WebDevelop/project-survey-1/scripts/logs/auto-update.log"
PLIST_LABEL="com.d9.survey-server"

mkdir -p "$(dirname "$LOG_FILE")"

log() {
    echo "[$(date '+%Y-%m-%d %H:%M:%S')] $1" >> "$LOG_FILE"
}

cd "$REPO_DIR" || { log "ERROR: 디렉토리 이동 실패: $REPO_DIR"; exit 1; }

# 현재 HEAD 커밋 해시 저장
LOCAL_HASH=$(git rev-parse HEAD)

# 원격 저장소에서 최신 정보 가져오기
git fetch origin 2>> "$LOG_FILE"
if [ $? -ne 0 ]; then
    log "ERROR: git fetch 실패"
    exit 1
fi

# 원격 브랜치의 최신 커밋 해시
BRANCH=$(git rev-parse --abbrev-ref HEAD)
REMOTE_HASH=$(git rev-parse "origin/$BRANCH")

# 변경사항이 없으면 종료
if [ "$LOCAL_HASH" = "$REMOTE_HASH" ]; then
    exit 0
fi

log "변경 감지! local=$LOCAL_HASH remote=$REMOTE_HASH"
log "git pull 실행 중..."

# 변경사항 pull
git pull origin "$BRANCH" 2>> "$LOG_FILE"
if [ $? -ne 0 ]; then
    log "ERROR: git pull 실패"
    exit 1
fi

log "git pull 완료. 서버 재시작 중..."

# 서버 재시작 (launchd 서비스를 unload 후 load — KeepAlive이므로 자동 재시작)
launchctl kickstart -k "gui/$(id -u)/$PLIST_LABEL" 2>> "$LOG_FILE"

if [ $? -eq 0 ]; then
    log "서버 재시작 완료"
else
    log "WARNING: kickstart 실패, bootout/bootstrap 시도..."
    launchctl bootout "gui/$(id -u)/$PLIST_LABEL" 2>> "$LOG_FILE"
    sleep 2
    launchctl bootstrap "gui/$(id -u)" "$HOME/Library/LaunchAgents/$PLIST_LABEL.plist" 2>> "$LOG_FILE"
    log "서버 재시작 완료 (bootout/bootstrap)"
fi
