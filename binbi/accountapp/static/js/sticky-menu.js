window.onscroll = function() {myFunction()};

var navbar = document.querySelector('.side-bar')
console.log(navbar);
var sticky = navbar;
console.log(sticky);

function myFunction() {
    if (window.scrollY>= sticky) {
      console.log(sticky);
      navbar.classList.add("sticky")
    } else {
        console.log(sticky);
      navbar.classList.remove("sticky");
    }
  }