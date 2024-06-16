from django.db import models
import datetime
from django.contrib.auth.models import User


class ReportResult(models.Model):
    name = models.CharField(max_length=100,default='report')
    defining_niche = models.JSONField(blank=True, default=dict)
    voluem_market = models.JSONField(blank=True, default=dict)
    list_key_players = models.JSONField(blank=True, default=dict)
    list_key_consumers = models.JSONField(blank=True, default=dict)
    list_common_products = models.JSONField(blank=True, default=dict)
    date_create = models.DateTimeField(default=datetime.datetime.now)
    is_active = models.BooleanField(default=False)
    user_id = models.ForeignKey(User, on_delete=models.CASCADE)

    def __str__(self):
        return self.name


class Template(models.Model):
    name = models.CharField(max_length=100)

    def __str__(self):
        return self.name
