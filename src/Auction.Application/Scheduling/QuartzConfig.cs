using System.Collections.Specialized;

namespace Auction.Application.Scheduling
{
    internal class QuartzConfig : Dictionary<string, string>
    {

        private const string QuartzConnectionStringSettingKey = "quartz.dataSource.quartzDS.connectionString";

        public QuartzConfig()
        {
            this["quartz.scheduler.instanceName"] = "Auction.Scheduling";
            this["quartz.scheduler.instanceId"] = "AUTO";
            this["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            this["quartz.threadPool.threadCount"] = "4";
            this["quartz.jobStore.misfireThreshold"] = "60000";
            this["quartz.serializer.type"] = "stj";
            this["quartz.jobStore.type"] = "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz";
            this["quartz.jobStore.useProperties"] = "false";
            this["quartz.jobStore.driverDelegateType"] = "Quartz.Impl.AdoJobStore.PostgreSQLDelegate, Quartz";
            this["quartz.jobStore.clustered"] = "true";
            this["quartz.jobStore.tablePrefix"] = "qrtz_";
            this["quartz.jobStore.dataSource"] = "quartzDS";
            this["quartz.dataSource.quartzDS.provider"] = "Npgsql";
        }

        public void SetConnectionString(string connectionString)
        {
            this[QuartzConnectionStringSettingKey] = connectionString;
        }

        public NameValueCollection ToNameValueCollection()
        {
            return this.Aggregate(new NameValueCollection(), (seed, current) =>
            {
                seed.Add(current.Key, current.Value);
                return seed;
            });
        }
    }
}
