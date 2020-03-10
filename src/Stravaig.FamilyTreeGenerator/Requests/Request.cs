using System;
using Paramore.Brighter;

namespace Stravaig.FamilyTreeGenerator.Requests
{
    public abstract class Request : IRequest
    {
        public Guid Id { get; set; }

        protected Request()
        {
            Id = Guid.NewGuid();
        }
    }
}