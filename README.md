HMAC.Sample
===========

Sample app built on ASP.NET Web API using HMAC for authentication and showing how an API can evolve over time

There's a few things to note:

1. There's a custom exception handler showing how to send an object back to the client as part of the error response
1. The /test page is used to show how JS code can interact with the various parts of the API.
1. The HMAC security stuff is in it's own library.  The api/values controller shows how you can call a secured API from C#.
1. The API versioning code has comments in it explaining what's what. The approach with versioning has been to version via the use of custom mime types, using the format of __application/vnd.sampleOrg.{resource}{.v2}{+json|+bson}__
1. I've not written many tests for this code. Sorry. It's sample code. (And, yes, I'm breaking my own rules)

Please remember that this is sample code and I use it to talk through concepts with people. It's NOT meant to be taken as is and dropped into your applications without you first understanding it and making changes as needed. You have been warned :-)

That said, if you find it useful, then that's great and I'm happy.
