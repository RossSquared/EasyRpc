﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace EasyRpc.AspNetCore.Middleware
{
    public abstract class BaseExposureConfiguration
    {
        protected readonly ICurrentApiInformation ApiInformation;
        protected readonly List<string> Names = new List<string>();
        protected readonly List<IMethodAuthorization> Authorizations = new List<IMethodAuthorization>();

        protected BaseExposureConfiguration(ICurrentApiInformation apiInformation)
        {
            ApiInformation = apiInformation;
        }

        protected IEnumerable<ExposedMethodInformation> GetExposedMethods(Type type,
            Func<MethodInfo, bool> methodFilter = null)
        {
            if (Names.Count == 0)
            {
                foreach (var routeName in ApiInformation.NamingConventions.RouteNameGenerator(type))
                {
                    Names.Add(routeName);
                }
            }

            return GetExposedMethods(type, ApiInformation, t => Names, Authorizations, methodFilter);
        }

        public static IEnumerable<ExposedMethodInformation> GetExposedMethods(Type type,
                                                                              ICurrentApiInformation currentApi,
                                                                              Func<Type, IEnumerable<string>> namesFunc,
                                                                              List<IMethodAuthorization> authorizations,
                                                                              Func<MethodInfo, bool> methodFilter)
        {
            var names = namesFunc(type).ToArray();

            IEnumerable<string> finalNames;

            if (currentApi.Prefixes.Count > 0)
            {
                var newNames = new List<string>();

                foreach (var prefixes in currentApi.Prefixes)
                {
                    foreach (var prefix in prefixes(type))
                    {
                        foreach (var name in names)
                        {
                            newNames.Add(prefix + name);
                        }
                    }
                }

                finalNames = newNames;
            }
            else
            {
                finalNames = names;
            }

            foreach (var authorization in currentApi.Authorizations)
            {
                foreach (var methodAuthorization in authorization(type))
                {
                    authorizations.Add(methodAuthorization);
                }
            }

            foreach (var method in type.GetRuntimeMethods())
            {
                if (method.IsStatic || !method.IsPublic)
                {
                    continue;
                }

                var filterOut = currentApi.MethodFilters.Any(func => !func(method));

                if (filterOut)
                {
                    continue;
                }

                if (methodFilter != null && !methodFilter(method))
                {
                    continue;
                }

                var filters = new List<Func<HttpContext, IEnumerable<ICallFilter>>>();

                foreach (var func in currentApi.Filters)
                {
                    var filter = func(method);

                    if (filter != null)
                    {
                        filters.Add(filter);
                    }
                }

                var currentAuth = new List<IMethodAuthorization>(authorizations);

                foreach (var attr in method.GetCustomAttributes<AuthorizeAttribute>())
                {
                    if (!string.IsNullOrEmpty(attr.Policy))
                    {
                        currentAuth.Add(new UserPolicyAuthorization(attr.Policy));
                    }
                    else if (!string.IsNullOrEmpty(attr.Roles))
                    {
                        currentAuth.Add(new UserRoleAuthorization(attr.Roles));
                    }
                    else
                    {
                        currentAuth.Add(new UserAuthenticatedAuthorization());
                    }
                }

                yield return new ExposedMethodInformation(type, finalNames, currentApi.NamingConventions.MethodNameGenerator(method), method, authorizations.ToArray(), filters.ToArray());
            }
        }
    }
}
