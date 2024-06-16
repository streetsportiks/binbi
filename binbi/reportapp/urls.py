from django.urls import path, include
from .views import ReportView, ReportsView
from .import views

urlpatterns = [
    path('', ReportsView.as_view(), name='reports_view'),
    path('single-report/<int:id_report>', ReportView.as_view(), name='report_view'),
    path('download_pdf/<int:report_id>', views.generate_pdf_view, name='download_pdf'),
]