from django.shortcuts import render
from django.views import View
from reportapp.models import ReportResult
from .tasks import get_module_analiz


class StartAnalizView(View):

    def get(self, request):
        return render(request, 'startanalizapp/start_analiz.html')

    def post(self, request):
        type_template = 'template_analiz'

        results = get_module_analiz(request.user,
                                    type_template=type_template,
                                    request_user=request)

        return render(request,
                      'startanalizapp/analiz_launched.html', )
