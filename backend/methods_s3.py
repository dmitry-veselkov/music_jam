import os

import boto3
from dotenv import load_dotenv


class S3SongMethods:
    @staticmethod
    def download_song(name: str) -> None:
        """
        Скачивание файла по имени
        :param name: полное название песни, включая расширение, которое лежит в хранилище
        """

        # if name.endswith(('.mp3', '.m4a', '.aac', '.wav'))
        load_dotenv()
        s3 = boto3.client(
            "s3",
            endpoint_url="https://storage.yandexcloud.net",
            aws_access_key_id=os.getenv('PUBLIC_KEY'),
            aws_secret_access_key=os.getenv('SECRET_KEY'),
        )
        s3.download_file("musicjam", name, name)
        print(f"Файл {name} успешно скачан в папку {os.getcwd()}")

    @staticmethod
    def upload_song(file_path: str, file_name: str) -> None:
        """
        Загрузка файла в хранилище
        :param file_path: абсолютный путь до файла
        :param file_name: название файла, включая расширение, который будет загружен в хранилище
        """
        load_dotenv()
        s3 = boto3.client(
            "s3",
            endpoint_url="https://storage.yandexcloud.net",
            aws_access_key_id=os.getenv('PUBLIC_KEY'),
            aws_secret_access_key=os.getenv('SECRET_KEY'),
        )
        s3.upload_file(file_path, "musicjam", file_name)
