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

  def login_as(self, user, password):
    url = 'https://account.gn.boberneprotiv.com/connect/token'
    data = {
      'client_id': 'api_tests',
      'client_secret': 'secret',
      'grant_type': 'password',
      'scope': 'profile api',
      'username': user,
      'password': password
    }

    return requests.post(url, data=data)