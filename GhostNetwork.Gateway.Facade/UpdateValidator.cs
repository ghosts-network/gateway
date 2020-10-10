using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GhostNetwork.Publications.Model;

namespace GhostNetwork.Gateway.Facade
{
    public class UpdateValidator : IUpdateValidator
    {
        private readonly int? maxTime;

        public UpdateValidator(int? maxTime)
        {
            this.maxTime = maxTime;
        }

        public bool TryUpdatePublication(Publication publication)
        {
            if (maxTime == null)
            {
                return true;
            }

            var time = DateTime.Now - publication.CreatedOn;
            
            if (time.TotalMinutes > maxTime)
            {
                return false;
            }

            return true;
        }
    }
}
