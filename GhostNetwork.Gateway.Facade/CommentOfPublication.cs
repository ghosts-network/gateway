using System;
using System.Collections.Generic;
using System.Text;

namespace GhostNetwork.Gateway.Facade
{
    public class CommentOfPublication
    {
        public CommentOfPublication(string content)
        {
            Content = content;
        }
        public string Content { get; set; }
    }
}
