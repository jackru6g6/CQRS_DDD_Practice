using MediatR;
using SampleProject.Domain.Domains.Aggregate;
using SampleProject.Domain.Interfaces.Domain;
using System.Collections;
using System.Reflection;

namespace SampleProject.Domain.Extensions
{
    /// <summary>
    /// 取出所有事件並推送執行
    /// </summary>
    public static class MediatorExtension
    {
        // 微軟範例
        //public static async Task DispatchDomainEventsAsync(this IMediator mediator, params Entity[] entities)
        //{
        //    var domainEntities = entities.Where(x => x.DomainEvents != null && x.DomainEvents.Any());

        //    // 取得即將要通知的事件
        //    var domainEvents = domainEntities.SelectMany(x => x.DomainEvents)
        //                                     .ToList();

        //    // 清除事件
        //    domainEntities.ToList()
        //                  .ForEach(entity => entity.ClearDomainEvents());

        //    foreach (var domainEvent in domainEvents)
        //    {
        //        await mediator.Publish(domainEvent);
        //    }
        //}

        public static async Task DispatchDomainEventsAsync(this IMediator mediator, params IAggregateRoot[] aggre)
        {
            var domainEntities = aggre.SelectMany(t => FindEntities(t))
                                      .Where(t => t.DomainEvents is not null && t.DomainEvents.Any())
                                      .ToList();

            // 取得即將要通知的事件
            var domainEvents = domainEntities.SelectMany(x => x.DomainEvents)
                                                             .ToList();

            // 清除事件
            domainEntities.ToList()
                          .ForEach(entity => entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }

        //private static bool InheritsFromEntity(Type type)
        //{
        //    Type entityType = typeof(AggregateRoot);
        //    return entityType.IsAssignableFrom(type);
        //}

        //public static IEnumerable<AggregateRoot> AAAA(object aggregate)
        //{
        //    Type type = aggregate.GetType();

        //    if (InheritsFromEntity(type))
        //    {
        //        yield return (AggregateRoot)aggregate;
        //    }

        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        var propType = prop.PropertyType;

        //        if (propType.IsArray )
        //        {
        //            Array arrayValue = (Array)prop.GetValue(aggregate);
        //            foreach (var item in arrayValue)
        //            {
        //                foreach (var a in AAAA(item))
        //                {
        //                    yield return a;
        //                }
        //            }
        //        }
        //        else if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(List<>))
        //        {
        //            IList listValue = (IList)prop.GetValue(aggregate);
        //            foreach (var item in listValue)
        //            {
        //                foreach (var a in AAAA(item))
        //                {
        //                    yield return a;
        //                }
        //            }
        //        }
        //        else if (InheritsFromEntity(prop.GetType()))
        //        {
        //            object value = prop.GetValue(aggregate);

        //            foreach (var a in AAAA(value))
        //            {
        //                yield return a;
        //            }
        //        }
        //    }
        //}


        public static IEnumerable<AggregateRoot> FindEntities(object obj)
        {
            Type entityType = typeof(AggregateRoot);
            Type objType = obj.GetType();

            if (entityType.IsAssignableFrom(objType))
            {
                yield return (AggregateRoot)obj;
            }

            foreach (PropertyInfo prop in objType.GetProperties())
            {
                var propType = prop.PropertyType;
                var propValue = prop.GetValue(obj);

                //if (propValue is IEnumerable enumerable && !(propValue is string))
                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    IEnumerable enumerable = (IEnumerable)prop.GetValue(obj);
                    if (enumerable is null)
                    {
                        continue;
                    }

                    foreach (var item in enumerable)
                    {
                        foreach (var entityItem in FindEntities(item))
                        {
                            yield return entityItem;
                        }
                    }
                }
                else if (propValue != null && entityType.IsAssignableFrom(propValue.GetType()))
                {
                    foreach (var entityItem in FindEntities(propValue))
                    {
                        yield return entityItem;
                    }
                }
            }
        }
    }
}
