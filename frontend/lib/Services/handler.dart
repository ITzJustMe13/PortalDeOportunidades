import 'package:http/http.dart' as http;
import 'package:flutter_dotenv/flutter_dotenv.dart';

abstract class Handler {
  final String apiIP;
  final http.Client client;

  Handler({
    http.Client? client,
    String? apiIP,
  })  : apiIP = 'https://localhost:7235',//apiIP ?? dotenv.env['API_KEY'] ?? 'https://localhost:7235',
        client = client ?? http.Client();
}