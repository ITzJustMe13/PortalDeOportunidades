import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Views/ActivationSucessScreen.dart';
import 'package:frontend/State/ActivationState.dart';
import 'package:http/http.dart' as http;

class InMemoryUserApiHandler extends UserApiHandler {
  final Map<String, bool> _responses = {};
   InMemoryUserApiHandler() : super(http.Client()); // Passe `null` ou um valor padrão apropriado

  @override
  Future<bool> activateAccount(String token) async {
    // Simula uma resposta de ativação sem fazer uma requisição HTTP real
    return token == 'dummy_token';
  }
  

  void addResponse(String token, bool success) {
    _responses[token] = success;
  }

}

void main() {
  group('ActivationSuccessPage Tests', () {
    late ActivationState activationState;

    setUp(() {
      // Inicia o estado com um handler padrão, mas não real
      activationState = ActivationState(
        apiHandler: UserApiHandler(http.Client()),
      );
    });

    testWidgets('Exibe estado inicial com botão "Ativar Conta"', (WidgetTester tester) async {
  final activationState = ActivationState();

  await tester.pumpWidget(
    MaterialApp(
      home: ChangeNotifierProvider<ActivationState>.value(
        value: activationState,
        child: ActivationSuccessPage(),
      ),
    ),
  );

  // Verifica se o botão "Ativar Conta" está visível
  expect(find.text('Ativar Conta'), findsOneWidget);
  expect(find.byIcon(Icons.error_outline), findsOneWidget);
});

    testWidgets('Exibe sucesso após ativação de conta', (WidgetTester tester) async {
      activationState.setAccountActivated(true);

      await tester.pumpWidget(
        MaterialApp(
          home: ChangeNotifierProvider.value(
            value: activationState,
            child: ActivationSuccessPage(),
          ),
        ),
      );

      expect(find.byIcon(Icons.check_circle_outline), findsOneWidget);
      expect(find.text('Conta Ativada!'), findsOneWidget);
    });

    testWidgets('Exibe mensagem de erro ao falhar na ativação', (WidgetTester tester) async {
      activationState.setErrorMessage('Erro ao ativar conta');

      await tester.pumpWidget(
        MaterialApp(
          home: ChangeNotifierProvider.value(
            value: activationState,
            child: ActivationSuccessPage(),
          ),
        ),
      );

      expect(find.text('Erro ao ativar conta'), findsOneWidget);
    });

    testWidgets('Chama ativação de conta ao pressionar o botão', (WidgetTester tester) async {
      activationState = ActivationState(
        apiHandler: UserApiHandler(http.Client()),
      );

      await tester.pumpWidget(
        MaterialApp(
          home: ChangeNotifierProvider.value(
            value: activationState,
            child: ActivationSuccessPage(token: 'dummy_token'),
          ),
        ),
      );

      await tester.tap(find.text('Ativar Conta'));
      await tester.pump();

      // Simula o resultado esperado
      expect(activationState.accountActivated, isTrue);
    });
  });
}

