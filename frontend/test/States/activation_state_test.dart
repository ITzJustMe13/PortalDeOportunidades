import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/State/ActivationState.dart';
import 'package:frontend/Services/user_api_handler.dart'; // Certifique-se de que este caminho está correto.
import 'package:http/http.dart' as http;


// Implementação em memória do UserApiHandler
class InMemoryUserApiHandler extends UserApiHandler {
  final Map<String, bool> _responses = {};

  InMemoryUserApiHandler() : super(http.Client());

  void addResponse(String token, bool success) {
    _responses[token] = success;
  }

  @override
  Future<bool> activateAccount(String token) async {
    return _responses[token] ?? false;
  }
}

void main() {
  group('ActivationState Tests', () {
    late InMemoryUserApiHandler apiHandler;
    late ActivationState activationState;

    setUp(() {
      apiHandler = InMemoryUserApiHandler();
      activationState = ActivationState();
      activationState.setApiHandler(apiHandler); // Sem cast explícito
    });

    test('Deve iniciar com valores padrão', () {
      expect(activationState.accountActivated, isFalse);
      expect(activationState.isLoading, isFalse);
      expect(activationState.errorMessage, isNull);
    });

    test('Deve definir errorMessage quando o token for vazio', () async {
      await activationState.activateAccount('');
      expect(activationState.accountActivated, isFalse);
      expect(activationState.errorMessage, 'Ocorreu um erro!');
      expect(activationState.isLoading, isFalse);
    });

    test('Deve ativar a conta com um token válido', () async {
      apiHandler.addResponse('valid-token', true);
      await activationState.activateAccount('valid-token');
      expect(activationState.accountActivated, isTrue);
      expect(activationState.errorMessage, isNull);
      expect(activationState.isLoading, isFalse);
    });

    test('Deve definir errorMessage quando a ativação falhar', () async {
      apiHandler.addResponse('invalid-token', false);
      await activationState.activateAccount('invalid-token');
      expect(activationState.accountActivated, isFalse);
      expect(activationState.errorMessage, 'Ocorreu um erro!');
      expect(activationState.isLoading, isFalse);
    });

    test('Deve atualizar isLoading corretamente durante o processo de ativação', () async {
      apiHandler.addResponse('valid-token', true);
      final activationFuture = activationState.activateAccount('valid-token');
      expect(activationState.isLoading, isTrue);
      await activationFuture;
      expect(activationState.isLoading, isFalse);
    });
  });
}