from django import forms
import datetime


class SearchReport(forms.Form):
    data_start = forms.DateField(initial=datetime.date.today, label='с',
                                 widget=forms.DateInput(attrs={'class': 'filed-datatime',
                                                               'type': 'date'}))
    data_end = forms.DateField(initial=datetime.date.today,label="по",
                               widget=forms.DateInput(attrs={'class': 'filed-datatime',
                                                             'type': 'date'}))
