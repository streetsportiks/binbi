# from io import BytesIO, StringIO
# from django.http import HttpResponse
# from django.template.loader import get_template
# from xhtml2pdf import pisa
# from django.conf import settings
# import os
#
#
# def render_to_pdf(template_src, context_dict={}):
#     template = get_template(template_src)
#     html = template.render(context_dict)
#     print(html)
#     result = BytesIO()
#     # pdf = pisa.CreatePDF(BytesIO(html.encode("UTF-8")), result, encoding='utf-8',
#     #                      link_callback=link)
#     pdf = pisa.CreatePDF(html,
#                          dest=result, encoding="UTF-8")
#     print(pdf)
#     if pdf.err:
#         return HttpResponse("Invalid PDF", status_code=400, content_type='text/plain')
#     return HttpResponse(result.getvalue(), content_type='application/pdf')

