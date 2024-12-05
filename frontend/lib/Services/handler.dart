import 'package:http/http.dart' as http;
import 'package:flutter_dotenv/flutter_dotenv.dart';

abstract class Handler {
  final String? apiIP;
  final http.Client client;

  Handler({http.Client? client})
      : apiIP =
            "https://portalodeportunidadesapi-emfdgkakb0fqchas.westeurope-01.azurewebsites.net",
        client = client ?? http.Client();
}