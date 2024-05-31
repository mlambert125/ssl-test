using System.Security.Cryptography.X509Certificates;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;

// Connect to Mongo with x509 certificatea
var connectionString = "mongodb://dockermain:27399/FusionCAC2?authSource=$external&authMechanism=MONGODB-X509";
var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
settings.UseTls = true;
settings.SslSettings.ClientCertificates = new List<X509Certificate2> { new("./dolbeyadmin.p12", "mongoClientPassword") };
settings.ServerApi = new ServerApi(ServerApiVersion.V1);
settings.AllowInsecureTls = true;
settings.Credential = MongoCredential.CreateMongoX509Credential("CN=dolbeyadmin");
var client = new MongoClient(settings);

// Get the database
var database = client.GetDatabase("FusionCAC2");
var accountCollection = database.GetCollection<BsonDocument>("codes");

// Get the first document
var account = accountCollection.Find(new BsonDocument()).FirstOrDefault();

// Print the document
Console.WriteLine(account);


// Rabbit test
var factory =
    new ConnectionFactory
    {
        Uri = new Uri("amqp://dockermain:5699/"),
        AutomaticRecoveryEnabled = true,
        AuthMechanisms = [new ExternalMechanismFactory()],
        Ssl = new SslOption
        {
            Enabled = true,
            ServerName = "dockermain",
            CertPath = "./app.p12",
            CertPassphrase = "rabbitClientPassword"
        }
    };

var rabbitConnection = factory.CreateConnection();
var rabbitChannel = rabbitConnection.CreateModel();
rabbitChannel.BasicQos(0, 10, false);


