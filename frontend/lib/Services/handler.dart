import 'package:http/http.dart' as http;

abstract class Handler {
  final String? apiIP;
  final http.Client client;

  Handler({http.Client? client})
      : apiIP = "https://localhost:7235",
           // "https://portalodeportunidadesapi-emfdgkakb0fqchas.westeurope-01.azurewebsites.net",
        client = client ?? http.Client();
}
