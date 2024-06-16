
window.onload = function(){
    let popupBg = document.querySelector('.popup__bg'); // Фон попап окна
    let popup = document.querySelector('.popup'); // Само окно
    let openPopupButtons = document.querySelectorAll('.open-popup'); // Кнопки для показа окна
    let closePopupButton = document.querySelector('.close-popup'); // Кнопка для скрытия окна
    let popupBg_2 = document.querySelector('.popup__bg-2'); // Фон попап окна
    let popup_2 = document.querySelector('.popup-2'); // Само окно
    let openPopupButtons_2 = document.querySelectorAll('.open-popup-2'); // Кнопки для показа окна
    let closePopupButton_2 = document.querySelector('.close-popup-2'); // Кнопка для скрытия окна

    if (popupBg){
        openPopupButtons.forEach((button) => { // Перебираем все кнопки
        button.addEventListener('click', (e) => { // Для каждой вешаем обработчик событий на клик
            e.preventDefault(); // Предотвращаем дефолтное поведение браузера
            popupBg.classList.add('active'); // Добавляем класс 'active' для фона
            popup.classList.add('active'); // И для самого окна
            })
        });

    };
    if (popupBg_2){
        openPopupButtons_2.forEach((button) => { // Перебираем все кнопки
        button.addEventListener('click', (e) => { // Для каждой вешаем обработчик событий на клик
            e.preventDefault(); // Предотвращаем дефолтное поведение браузера
            popupBg_2.classList.add('active'); // Добавляем класс 'active' для фона
            popup_2.classList.add('active'); // И для самого окна
            })
        });
    };


    closePopupButton.addEventListener('click',() => { // Вешаем обработчик на крестик
        popupBg.classList.remove('active'); // Убираем активный класс с фона
        popup.classList.remove('active'); // И с окна
    });

    closePopupButton_2.addEventListener('click',() => { // Вешаем обработчик на крестик
        popupBg_2.classList.remove('active'); // Убираем активный класс с фона
        popup_2.classList.remove('active'); // И с окна
    });

    document.addEventListener('click', (e) => { // Вешаем обработчик на весь документ
        if(e.target === popupBg) { // Если цель клика - фот, то:
            popupBg.classList.remove('active'); // Убираем активный класс с фона
            popup.classList.remove('active'); // И с окна
        }
        else if(e.target === popupBg_2){ // Если цель клика - фот, то:
            popupBg_2.classList.remove('active'); // Убираем активный класс с фона
            popup_2.classList.remove('active'); // И с окна
        }
    });
}



