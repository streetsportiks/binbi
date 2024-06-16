FROM python:3.10.6

ENV PYTHONDONTWRITEBYCODE=1
ENV PYTHONUNBUFFERED=1

WORKDIR /code

RUN pip install --upgrade pip
COPY requrement.txt /code/
RUN pip install -r requrement.txt

COPY . /code/

