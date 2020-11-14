import requests
import json

class NewsFeedApi:
  base_url = 'http://localhost:5000/newsfeed/'
  headers = {'Content-Type': 'application/json'}

  def create_publication(self, body):
    return requests.post(self.base_url, headers=self.headers, data=json.dumps(body))

  def get_publication_by_id(self, id):
    url = self.base_url + id + '/'
    return requests.get(url, headers=self.headers)
