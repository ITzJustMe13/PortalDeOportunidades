import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

abstract class Handler {
  final String apiIP;
  final http.Client client;

  Handler({
    http.Client? client,
    //this.apiIP = "https://portalodeportunidadesapi-emfdgkakb0fqchas.westeurope-01.azurewebsites.net/",
    this.apiIP = "https://localhost:7235",
  }) : client = client ?? http.Client();
}