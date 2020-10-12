using System;
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

        public DomainResult CanUpdatePublication(Publication publication)
        {
            if (maxTime == null)
            {
                return DomainResult.Successed();
            }

            var time = DateTime.Now - publication.CreatedOn;
            
            if (time.TotalMinutes > maxTime)
            {
                return DomainResult.Error("Unable to update publication.");
            }

            return DomainResult.Successed();
        }
    }
}
