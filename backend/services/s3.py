import boto3
from uuid import uuid4


class S3:
    _BUCKET_NAME = "musicjam"
    _ENDPOINT_URL = "https://storage.yandexcloud.net"

    def __init__(self, public_key: str, secret_key: str):
        self.s3 = boto3.client(
            "s3",
            endpoint_url=self._ENDPOINT_URL,
            aws_access_key_id=public_key,
            aws_secret_access_key=secret_key
        )

    def upload(self, file, file_name: str, content_type: str):
        self.s3.upload_fileobj(file, "musicjam", file_name, ExtraArgs={"ContentType": content_type})

    def get_stream(self, file_key: str):
        response = self.s3.get_object(Bucket=self._BUCKET_NAME, Key=file_key)
        return response['Body'], response['ContentType']

    @staticmethod
    def get_unique_s3_uuid(name: str):
        return f"{uuid4()}_{name}"
