import 'package:flutter/material.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/State/ActivationState.dart';
import 'package:provider/provider.dart';

class ActivationSuccessPage extends StatelessWidget {
  final String? token;

  const ActivationSuccessPage({super.key, this.token});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: CustomAppBar(),
        endDrawer: CustomDrawer(),
        body: Consumer<ActivationState>(
            builder: (context, activationState, child) {
          return activationState.accountActivated
              ? _accountActivated(context)
              : _activateAccount(context, activationState, token);
        }));
  }
}

Widget _activateAccount(context, activationState, String? token) {
  return Center(
    child: Padding(
      padding: const EdgeInsets.all(20.0),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          !activationState.accountActivated
              ? Icon(
                  Icons.error_outline,
                  color: Colors.green,
                  size: 100,
                )
              : CircularProgressIndicator(),
          const SizedBox(height: 20),
          Text(
            'Ativar Conta!',
            style: TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
              color: Colors.black,
            ),
          ),
          const SizedBox(height: 30),
          ElevatedButton(
            onPressed: () {
              activationState.activateAccount(token ?? "");
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.green,
              padding: const EdgeInsets.symmetric(horizontal: 40, vertical: 15),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(30),
              ),
            ),
            child: const Text(
              'Ativar Conta',
              style: TextStyle(fontSize: 16),
            ),
          ),
          if (activationState.errorMessage != null)
            Text(
              activationState.errorMessage!,
              style: TextStyle(color: Colors.red),
            ),
        ],
      ),
    ),
  );
}

Widget _accountActivated(context) {
  return Center(
    child: Padding(
      padding: const EdgeInsets.all(20.0),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.check_circle_outline,
            color: Colors.green,
            size: 100,
          ),
          const SizedBox(height: 20),
          Text(
            'Conta Ativada!',
            style: TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
              color: Colors.black,
            ),
          ),
          const SizedBox(height: 10),
          Text(
            'A sua conta foi ativada com sucesso. Agora você pode aproveitar todas as nossas funcionalidades!',
            textAlign: TextAlign.center,
            style: TextStyle(
              fontSize: 16,
              color: Colors.grey[700],
            ),
          ),
          const SizedBox(height: 30),
          ElevatedButton(
            onPressed: () {
              // Navigate to another page, e.g., Home or Login
              Navigator.pushReplacementNamed(context, '/login');
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.green,
              padding: const EdgeInsets.symmetric(horizontal: 40, vertical: 15),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(30),
              ),
            ),
            child: const Text(
              'Ir para o Login',
              style: TextStyle(fontSize: 16),
            ),
          ),
          const SizedBox(height: 30),
          ElevatedButton(
            onPressed: () {
              // Navigate to another page, e.g., Home or Login
              Navigator.pushReplacementNamed(context, '/');
            },
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.green,
              padding: const EdgeInsets.symmetric(horizontal: 40, vertical: 15),
              shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(30),
              ),
            ),
            child: const Text(
              'Ir para o Início',
              style: TextStyle(fontSize: 16),
            ),
          ),
        ],
      ),
    ),
  );
}
