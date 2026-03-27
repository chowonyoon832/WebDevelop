document.addEventListener('DOMContentLoaded', function () {
    var form = document.getElementById('survey-form');
    var submitBtn = document.getElementById('submit-btn');
    var submitHint = document.getElementById('submit-hint');
    var answeredCount = document.getElementById('answered-count');
    var progressFill = document.getElementById('progress-fill');
    var radios = form.querySelectorAll('input[type="radio"]');
    var totalQuestions = 14;

    function updateProgress() {
        var answered = 0;
        for (var i = 1; i <= totalQuestions; i++) {
            if (form.querySelector('input[name="answer_' + i + '"]:checked')) {
                answered++;
            }
        }

        answeredCount.textContent = answered;
        progressFill.style.width = (answered / totalQuestions * 100) + '%';

        if (answered === totalQuestions) {
            submitBtn.disabled = false;
            submitHint.style.display = 'none';
        } else {
            submitBtn.disabled = true;
            submitHint.style.display = 'block';
        }
    }

    for (var i = 0; i < radios.length; i++) {
        radios[i].addEventListener('change', updateProgress);
    }
});
