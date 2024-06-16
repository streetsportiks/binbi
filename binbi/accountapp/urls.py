from django.urls import path, include
from .views import StartPageView
from . import views


urlpatterns = [

    # url стартовой страницы
    path('', include('django.contrib.auth.urls')),
    path('', StartPageView.as_view(), name='dashboard'),
]