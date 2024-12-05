import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/State/ActivationState.dart';
import 'package:mockito/mockito.dart';

import '../mocks.mocks.dart';

void main() {
  group('ActivationState Tests', () {
    late MockUserApiHandler apiHandler;
    late ActivationState activationState;

    setUp(() {
      apiHandler = MockUserApiHandler();
      activationState = ActivationState();
      activationState.setApiHandler(apiHandler); // Usando setter para injeção de dependência
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
      // Simulando sucesso com um token válido
      when(apiHandler.activateAccount('valid-token')).thenAnswer((_) async => true);

      await activationState.activateAccount('valid-token');
      expect(activationState.accountActivated, isTrue);
      expect(activationState.errorMessage, isNull);
      expect(activationState.isLoading, isFalse);
    });

    test('Deve definir errorMessage quando a ativação falhar', () async {
      // Simulando falha com um token inválido
      when(apiHandler.activateAccount('invalid-token')).thenAnswer((_) async => false);

      await activationState.activateAccount('invalid-token');
      expect(activationState.accountActivated, isFalse);
      expect(activationState.errorMessage, 'Ocorreu um erro!');
      expect(activationState.isLoading, isFalse);
    });

    test('Deve atualizar isLoading corretamente durante o processo de ativação', () async {
      // Simulando sucesso com um token válido
      when(apiHandler.activateAccount('valid-token')).thenAnswer((_) async => true);

      final activationFuture = activationState.activateAccount('valid-token');
      expect(activationState.isLoading, isTrue);
      await activationFuture;
      expect(activationState.isLoading, isFalse);
    });
  });
}