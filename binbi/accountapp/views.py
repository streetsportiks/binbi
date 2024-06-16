from django.shortcuts import render
from django.views import View
from django.contrib.auth.decorators import login_required
from .forms import UserRegistrationForm, UserLogin
import logging

logger = logging.getLogger(__name__)





class StartPageView(View):

    def get(self, request):
        user_form_registration = UserRegistrationForm()
        user_form_login = UserLogin()
        return render(request, 'base.html',
                      {'user_form': user_form_registration,
                       'user_form_login': user_form_login})

    def post(self, request):
        user_form = UserRegistrationForm(request.POST)
        if user_form.is_valid():
            # Создаём обьект пользователя но пока не сохраняем его
            new_user = user_form.save(commit=False)
            # Устанавливаем пароль
            new_user.set_password(
                user_form.cleaned_data['password'])
            # Сохраняем пользователя
            new_user.save()
            return render(request,
                          'base.html',
                          {'new_user': new_user})
        else:
            user_form = UserRegistrationForm()
        return render(request,
                      'base.html',
                      {'user_form': user_form})
