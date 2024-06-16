from celery import shared_task
import requests


@shared_task
def get_module_analiz(id_user, type_template, request_user):
    r = requests.get('http://module_parser:8001/data_analiz/'+ id_user + '/'
                     + type_template + '/' + request_user)

    if r.status_code == 200:
        return r.json()
