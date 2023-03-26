using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glash.Client.WinForm.Core
{
    public class MessageTitleAndTextException : ApplicationException
    {
        public string Title { get; set; }
        public string Text { get; set; }

        public MessageTitleAndTextException(string title, string text)
        {
            Title = title;
            Text = text;
        }
    }
}
