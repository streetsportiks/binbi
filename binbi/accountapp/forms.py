from django import forms
from django.contrib.auth.models import User


class UserLogin(forms.ModelForm):

    email = forms.EmailField(label="Email", widget=forms.EmailInput
    (attrs={'class': 'input-field', 'placeholder': 'user@mail.ru'}))
    password = forms.CharField(label='Пароль',
                               widget=forms.PasswordInput
                               (attrs={'class': 'input-field'}))

    class Meta:
        model = User
        fields = ['email']


class UserRegistrationForm(forms.ModelForm):
    username = forms.CharField(label="Название Организации", widget=forms.TextInput
    (attrs={'placeholder': 'Введите название огранизации', 'class': 'input-field'}))

    email = forms.EmailField(label="Email", widget=forms.EmailInput
    (attrs={'class': 'input-field', 'placeholder': 'user@mail.ru'}))
    password = forms.CharField(label='Пароль',
                               widget=forms.PasswordInput
                               (attrs={'class': 'input-field'}))
    password2 = forms.CharField(label='Повторите пароль',
                                widget=forms.PasswordInput
                                (attrs={'class': 'input-field'}))

    class Meta:
        model = User
        fields = ['username', 'email']

    def clean_password2(self):
        cd = self.cleaned_data
        if cd['password'] != cd['password2']:
            raise forms.ValidationError('Пароли не совпадают.')
        return cd['password2']

    def clean_email(self):
        data = self.cleaned_data['email']
        if User.objects.filter(email=data).exists():
            raise forms.ValidationError('Такой email уже существует')
        return data


class UserEditForm(forms.ModelForm):
    class Meta:
        model = User
        fields = ['first_name', 'last_name', 'email']

    def clean_email(self):
        data = self.cleaned_data['email']
        qs = (User.objects.exclude(id=self.instance.id).filter(email=data))
        if qs.exists():
            raise forms.ValidationError('Такой e-mail уже существует')
        return data
