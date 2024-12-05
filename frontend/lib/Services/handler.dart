import 'package:http/http.dart' as http;
import 'package:flutter_dotenv/flutter_dotenv.dart';

abstract class Handler {
  final String? apiIP;
  final http.Client client;

  Handler({http.Client? client})
      : apiIP = dotenv.env["API_IP"],
        client = client ?? http.Client();
}
