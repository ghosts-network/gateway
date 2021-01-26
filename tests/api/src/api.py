import requests
import json

class NewsFeedApi:
  base_url = 'http://localhost:5000/newsfeed/'
  user = None

  def get_headers(self):
    headers = {'Content-Type': 'application/json'}
    if not (self.user is None):
      headers['Authorization'] = 'Bearer ' + self.user['access_token']

    return headers

  def post_publication(self, body):
    return requests.post(self.base_url, headers=self.get_headers(), data=json.dumps(body))

  def put_publication(self, id, body):
    url = self.base_url + id + '/'
    return requests.put(url, headers=self.get_headers(), data=json.dumps(body, indent=4))

  def delete_publication(self, id):
    url = self.base_url + id + '/'
    return requests.delete(url, headers=self.get_headers())

  def get_comments_by_publication_id(self, publication_id):
    url = self.base_url + publication_id + '/comments'
    return requests.get(url, headers=self.get_headers())

  def post_comment(self, publication_id, body):
    url = self.base_url + publication_id + '/comments'
    return requests.post(url, headers=self.get_headers(), data=json.dumps(body))

  def delete_comment(self, id):
    url = self.base_url + 'comments/' + id
    return requests.delete(url, headers=self.get_headers())

  def post_reaction(self, publication_id, body):
    url = self.base_url + publication_id + '/reaction'
    return requests.post(url, headers=self.get_headers(), data=json.dumps(body))

  def delete_reaction(self, publication_id):
    url = self.base_url + publication_id + '/reaction'
    return requests.delete(url, headers=self.get_headers())

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

  def logout(self):
    self.user = None
