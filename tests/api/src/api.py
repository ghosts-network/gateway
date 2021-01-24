import requests
import json

class NewsFeedApi:
  base_url = 'http://localhost:5000/newsfeed/'
  headers = {'Content-Type': 'application/json'}

  def post_publication(self, body):
    self.headers['Authorization'] = 'Bearer ' + self.user['access_token']
    return requests.post(self.base_url, headers=self.headers, data=json.dumps(body))

  def put_publication(self, id, body):
    self.headers['Authorization'] = 'Bearer ' + self.user['access_token']
    url = self.base_url + id + '/'
    return requests.put(url, headers=self.headers, data=json.dumps(body, indent=4))

  def delete_publication(self, id):
    self.headers['Authorization'] = 'Bearer ' + self.user['access_token']
    url = self.base_url + id + '/'
    return requests.delete(url, headers=self.headers)

  def get_comments_by_publication_id(self, publication_id):
    self.headers['Authorization'] = 'Bearer ' + self.user['access_token']
    url = self.base_url + publication_id + '/comments'
    return requests.get(url, headers=self.headers)

  def post_comment(self, publication_id, body):
    self.headers['Authorization'] = 'Bearer ' + self.user['access_token']
    url = self.base_url + publication_id + '/comments'
    return requests.post(url, headers=self.headers, data=json.dumps(body))

  def delete_comment(self, id):
    self.headers['Authorization'] = 'Bearer ' + self.user['access_token']
    url = self.base_url + 'comments/' + id
    return requests.delete(url, headers=self.headers)

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

    response = requests.post(url, data=data)
    self.user = response.json()
    return response
