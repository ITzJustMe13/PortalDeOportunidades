import 'package:google_sign_in/google_sign_in.dart';
import 'dart:convert';
import 'package:http/http.dart' as http;

// Instância do GoogleSignIn para Web
final GoogleSignIn _googleSignIn = GoogleSignIn(
  clientId: '348791959445-mo2sbugjbvmebd4ag0usmlupff0gv35l.apps.googleusercontent.com',  // Substitua pela sua Client ID da Web
);

class AuthService {
  Future<void> signInWithGoogle() async {
    try {
      final GoogleSignInAccount? googleUser = await _googleSignIn.signIn();
      if (googleUser == null) {
        // Login cancelado pelo usuário
        return;
      }

      final GoogleSignInAuthentication googleAuth = await googleUser.authentication;

      final String? idToken = googleAuth.idToken;

      if (idToken != null) {
        await sendIdTokenToBackend(idToken);
      }
    } catch (error) {
      print("Erro ao fazer login com Google: $error");
    }
  }

  Future<void> sendIdTokenToBackend(String idToken) async {
    final response = await http.post(
      Uri.parse('https://sua-api.com/google-login'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'idToken': idToken}),
    );

    if (response.statusCode == 200) {
      print("Login bem-sucedido!");
    } else {
      print("Erro no login: ${response.body}");
    }
  }
}
