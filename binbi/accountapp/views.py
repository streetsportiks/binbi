from django.shortcuts import render
from django.contrib.auth.decorators import login_required
from .forms import UserRegistrationForm


@login_required
def dashboard(request):
    return render(request, 'account/dashboard.html',
                  {'section': 'dashboard'})


def register(request):
    if request.method == 'POST':
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
                          'account/template/register_done.html',
                          {'new_user': new_user})
    else:
        user_form = UserRegistrationForm()
    return render(request,
                  'account/template/register.html',
                  {'user_form': user_form})
