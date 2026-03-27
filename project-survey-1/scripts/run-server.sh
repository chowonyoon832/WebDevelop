#!/bin/bash
# project-survey-1 서버 실행 스크립트
# launchd에 의해 관리됨 — 프로세스가 종료되면 자동으로 재시작됩니다.

PROJECT_DIR="/Users/d9/Documents/syncthing-d9/github/WebDevelop/project-survey-1/ConstructionSurvey"
LOG_DIR="/Users/d9/Documents/syncthing-d9/github/WebDevelop/project-survey-1/scripts/logs"

mkdir -p "$LOG_DIR"

echo "[$(date '+%Y-%m-%d %H:%M:%S')] 서버 시작" >> "$LOG_DIR/server.log"

cd "$PROJECT_DIR"
exec /usr/local/share/dotnet/dotnet run --launch-profile http 2>&1 | tee -a "$LOG_DIR/server.log"
