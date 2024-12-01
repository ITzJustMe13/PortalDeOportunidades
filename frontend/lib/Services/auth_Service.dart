import 'package:google_sign_in/google_sign_in.dart';
import 'dart:convert';
import 'package:http/http.dart' as http;

// Instância do GoogleSignIn para Web
final GoogleSignIn _googleSignIn = GoogleSignIn(
  clientId: '348791959445-mo2sbugjbvmebd4ag0usmlupff0gv35l.apps.googleusercontent.com ',
  scopes: <String>[
    'email',
    'profile',
    'openid',
    'https://www.googleapis.com/auth/userinfo.email',
    'https://www.googleapis.com/auth/userinfo.profile',
  ],
);

class AuthService {
  Future<bool> signInWithGoogle() async {
  try {
    // Inicia o processo de login com o Google
    final GoogleSignInAccount? googleUser = await _googleSignIn.signIn();

    if (googleUser == null) {
      return false;  // Cancelado pelo usuário
    }

    // Obtenção dos dados de autenticação
    final GoogleSignInAuthentication googleAuth = await googleUser.authentication;
    final String? idToken = googleAuth.idToken;

    if (idToken != null) {
      // Aguarda um pequeno tempo para garantir que a janela pop-up seja fechada
      await Future.delayed(Duration(seconds: 1));

      // Envia o ID token para o backend
      bool isLoggedIn = await sendIdTokenToBackend(idToken);
      return isLoggedIn;
    }

    return false;  // Se não houver idToken, retorna false
  } catch (error) {
    print("Erro ao fazer login com Google: $error");
    return false;
  }
}

  Future<bool> sendIdTokenToBackend(String idToken) async {
    final response = await http.post(
      Uri.parse('https://localhost:7235/api/User/google-sign-in'),
      headers: {'Content-Type': 'application/json'},
      body: jsonEncode({'idToken': idToken}),
    );

    if (response.statusCode == 200) {
      print("Login bem-sucedido!");
      return true;
    } else {
      print("Erro no login: ${response.body}");
      return false;
    }
  }
}
