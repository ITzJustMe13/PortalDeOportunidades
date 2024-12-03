
import 'package:http/http.dart' as http;

abstract class handler{
  String apiIP;
 final http.Client client=http.Client();

  handler({ this.apiIP = "https://portalodeportunidadesapi-emfdgkakb0fqchas.westeurope-01.azurewebsites.net/"});
   
}
