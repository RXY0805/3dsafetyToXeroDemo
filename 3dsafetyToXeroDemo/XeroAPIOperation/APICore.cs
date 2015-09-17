using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using _3dsafetyToXeroDemo.Xero3dSafetySetting;

using Xero.Api.Core;
using Xero.Api.Infrastructure.OAuth;
using Xero.Api.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace _3dsafetyToXeroDemo.XeroAPIOperation
{
    public class APICore:XeroCoreApi
    {
        private static readonly DefaultMapper Mapper = new DefaultMapper();
        private static readonly ConsumerSetting ApplicationSettings = new ConsumerSetting();
        private static readonly X509Certificate2 cert = new X509Certificate2(ApplicationSettings.SigningCertificatePath, "3dsafety");

        public APICore() :
            base(ApplicationSettings.Uri,
                new PrivateAuthenticator(cert),
                new Consumer(ApplicationSettings.Key, ApplicationSettings.Secret),
                null,
                Mapper,
                Mapper)
        {
        }
    }
}
