var body = document.getElementsByTagName('body')[0];
body.style.backgroundColor = '#111111';

var additionalSectionHeaderElement = document.getElementById('additionalSectionHeader');
var additionalSectionParagraph = document.getElementById('additionalSectionParagraph');

var statisticsHeaderElement = document.getElementById('statisticsHeader');

var statisticCardsElements = document.getElementsByClassName('card-overlay');
var arr = Array.from(statisticCardsElements);


// trigger this function every time the user scrolls
window.onscroll = function (event) {
    var scroll = window.pageYOffset;
    if (scroll < 569) {
        // dark grey
        body.style.backgroundColor = '#111111';
        additionalSectionHeaderElement.style.backgroundColor = '#ff4446';
        additionalSectionParagraph.style.color = '#CACACA';

        statisticsHeaderElement.style.backgroundColor = '#ff4446';
        statisticsHeaderElement.style.borderRadius = '20px';
        statisticsHeaderElement.style.height = "50px";

        arr.forEach(element => {

            var progressBarElement = element.getElementsByClassName('l-bg-cyan');
            var progressArr = Array.from(progressBarElement);

            if (progressArr[0].classList.contains('progress-bar-black')) {
                progressArr[0].classList.remove('progress-bar-black');
                progressArr[0].classList.add('progress-bar-red');
            }

            if (element.classList.contains('card-overlay')) {
                element.classList.remove('card-overlay');
                element.classList.add('card-overlay-black');
            }
        });
    } else if (scroll >= 569) {
        // white
        body.style.backgroundColor = 'white';
        additionalSectionHeaderElement.style.backgroundColor = 'white';
        additionalSectionParagraph.style.color = '#111111';

        statisticsHeaderElement.style.backgroundColor = 'white';

        arr.forEach(element => {
            var progressBarElement = element.getElementsByClassName('l-bg-cyan');
            var progressArr = Array.from(progressBarElement);

            if (progressArr[0].classList.contains('progress-bar-red')) {
                progressArr[0].classList.remove('progress-bar-red');
                progressArr[0].classList.add('progress-bar-black');
            }

            if (element.classList.contains('card-overlay-black')) {
                element.classList.remove('card-overlay-black');
                element.classList.add('card-overlay');
            }
        });
    }
}