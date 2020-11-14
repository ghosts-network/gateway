from src.api import NewsFeedApi

class TestNewsFeed(NewsFeedApi):
  def test_correct_publication(self):
    content = 'My first publication #awesome'

    resp = self.create_publication({'content': content})
    assert resp.status_code == 201

    resp_body = resp.json()
    assert 'id' in resp_body
    assert resp_body['content'] == content
    assert resp_body['comments']['totalCount'] == 0
    assert len(resp_body['comments']['topComments']) == 0
    assert resp_body['reactions']['totalCount'] == 0
    assert len(resp_body['reactions']['reactions']) == 0
