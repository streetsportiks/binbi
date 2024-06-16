from django.shortcuts import render
from django.http import Http404
from django.views import View
from .models import Template, ReportResult
from .forms import SearchReport
from django.http import HttpResponse
from weasyprint import HTML
from django.template.loader import render_to_string


class ReportsView(View):

    def get(self, request):
        search_report_form = SearchReport()
        template = Template.objects.all().first()
        # выводим только результыта пользователя
        reports = ReportResult.objects.filter(user_id=request.user)
        return render(request,
                      'reportapp/reports_list_bd.html',
                      {'template': template,
                       'reports': reports,
                       'search_report_form': search_report_form})

    def post(self, request):
        search_report_form = SearchReport(request.POST)
        if search_report_form.is_valid():
            template = Template.objects.all().first()
            reports = ReportResult.objects.filter(user_id=request.user)
            search_report_check = []
            for report in reports:
                if (search_report_form.cleaned_data.get("data_start") <= report.date_create.date()
                        <= search_report_form.cleaned_data.get("data_end")):
                    search_report_check.append(report)

                return render(request,
                              'reportapp/reports_list_bd.html',
                              {'template': template,
                               'reports': search_report_check,
                               'search_report_form': search_report_form})


class ReportView(View):

    def get(self, request, id_report):
        report = ReportResult.objects.filter(pk=id_report).first()
        template = Template.objects.all().first()
        labels_volem_gagr = []
        data_volem_gagr = []
        labels_volem_common = []
        data_volem_common = []
        labels_volem_type = []
        data_volem_type = []
        labels_key_company_world = []
        data_key_company_world = []
        labels_key_company_rus = []
        data_key_company_rus = []
        labels_key_consumers = []
        data_key_consumers = []





        for key, val in report.voluem_market.items():
            if key == "CAGR":
                data_volem_gagr.append(100)
                labels_volem_gagr.append("Общий обьём рынка")
                labels_volem_gagr.append(key)
                data_volem_gagr.append(val.replace("%", '').split("-")[1])

        for key, val in report.voluem_market.items():
            if key == "voluem_market_type":
                split_val = val.replace(":","")
                for i in split_val.split(" "):
                    if i.isdigit():
                        if len(i) == 4:
                            labels_volem_common.append(i)
                        else:
                            data_volem_common.append(i)

        for key,val in report.voluem_market.items():
            if key == 'structure_by_segment':
                split_val = val.split("%")
                for i in split_val:
                    data_volem_type.append(i.split(":")[-1].split("-")[-1])
                    labels_volem_type.append(i.split(":")[0].split("-")[-1])

        for key, val in report.list_key_players.items():
            if key == 'key_brends':
                split_val = val.split("%")
                for i in split_val:
                    data_key_company_world.append(i.split(":")[-1])
                    if i.split(":")[0].split():
                        labels_key_company_world.append(i.split(":")[0].split()[-1])

        for key, val in report.list_key_players.items():
            if key == 'key_brends_rus':
                split_val = val.split("%")
                for i in split_val:
                    data_key_company_rus.append(i.split(":")[-1])
                    if i.split(":")[0].split():
                        labels_key_company_rus.append(i.split(":")[0].split()[-1])

        for key, val in report.list_key_consumers.items():
            if key == 'key_cons':
                split_val = val.split("%")
                for i in split_val:
                    data_key_consumers.append(i.split(":")[-1])
                    if i.split(":")[0].split():
                        labels_key_consumers.append(i.split(":")[0].replace("- ",""))



        list_report = ['defining_niche',
                       'voluem_market',
                       'list_key_players',
                       'key_consumers',
                       'development_trends']
        return render(request, 'reportapp/single_reports.html',
                      {'report': report,
                       'template': template,
                       'list_report': list_report,
                       'labels_gagr': labels_volem_gagr,
                       'labels_volem_common': labels_volem_common,
                       'data_volem_common': data_volem_common,
                       'data_gagr': data_volem_gagr,
                       'labels_volem_type':labels_volem_type,
                       'data_volem_type': data_volem_type,
                       'labels_key_company_world':labels_key_company_world,
                       'data_key_company_world': data_key_company_world,
                       'labels_key_company_rus': labels_key_company_rus,
                       'data_key_company_rus': data_key_company_rus,
                       'labels_key_consumers': labels_key_consumers,
                       'data_key_consumers': data_key_consumers,


                       })


def generate_pdf_view(request, report_id):
    report = ReportResult.objects.filter(pk=report_id).first()
    template = Template.objects.all().first()

    list_report = ['defining_niche',
                   'voluem_market',
                   'list_key_players',
                   'key_consumers',
                   'development_trends']

    html_template = render_to_string('pdf/statement_form.html',
                                     {'report': report,
                                      'template': template,
                                      'list_report': list_report})

    html = HTML(string=html_template)
    pdf = html.write_pdf()
    response = HttpResponse(pdf, content_type='application/pdf')
    response['Content-Disposition'] = 'attachment; filename="output.pdf"'
    return response


