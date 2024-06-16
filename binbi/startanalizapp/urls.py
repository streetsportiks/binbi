from django.urls import path, include
from .views import StartAnalizView


urlpatterns = [
    path('', StartAnalizView.as_view(), name='start_analiz'),
]