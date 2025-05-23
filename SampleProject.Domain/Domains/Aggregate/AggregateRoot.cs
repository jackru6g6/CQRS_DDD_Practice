﻿using MediatR;
using SampleProject.Domain.Interfaces.Domain;

namespace SampleProject.Domain.Domains.Aggregate
{
    public abstract class AggregateRoot : IAggregateRoot
    {
        private int? _requestedHashCode;
        private int _Id;

        public virtual int Id
        {
            get
            {
                return _Id;
            }
            protected set
            {
                _Id = value;
            }
        }

        /// <summary>
        /// 通知事件
        /// </summary>
        /// <remarks>
        /// 微軟範例是在 Entity 中，但我覺得應該放在 AggregateRoot，因為 Entity 可能會有多個，但 AggregateRoot 只有一個。
        /// </remarks>
        private List<INotification> _domainEvents;

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        /// <summary>
        /// 加入通知事件
        /// </summary>
        /// <remarks>
        /// 故意用 protected，避免外部直接呼叫，但可以由聚合自己呼叫
        /// </remarks>
        /// <param name="eventItem"></param>
        protected void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem) => _domainEvents?.Remove(eventItem);

        public void ClearDomainEvents() => _domainEvents?.Clear();

        //public bool IsTransient()
        //{
        //    return Id == default;
        //}

        //public override bool Equals(object obj)
        //{
        //    if (obj == null || !(obj is AggregateRoot))
        //        return false;

        //    if (ReferenceEquals(this, obj))
        //        return true;

        //    if (GetType() != obj.GetType())
        //        return false;

        //    AggregateRoot item = (AggregateRoot)obj;

        //    if (item.IsTransient() || IsTransient())
        //        return false;
        //    else
        //        return item.Id == Id;
        //}

        //public override int GetHashCode()
        //{
        //    if (!IsTransient())
        //    {
        //        if (!_requestedHashCode.HasValue)
        //            _requestedHashCode = Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

        //        return _requestedHashCode.Value;
        //    }
        //    else
        //        return base.GetHashCode();

        //}

        //public static bool operator ==(AggregateRoot left, AggregateRoot right)
        //{
        //    if (Equals(left, null))
        //        return Equals(right, null) ? true : false;
        //    else
        //        return left.Equals(right);
        //}

        //public static bool operator !=(AggregateRoot left, AggregateRoot right)
        //{
        //    return !(left == right);
        //}
    }
}
