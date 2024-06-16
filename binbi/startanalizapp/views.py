from django.shortcuts import render
from django.views import View

class StartAnalizView(View):

    def get(self, request):
        return render(request,'startanalizapp/start_analiz.html')
