import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/Enums/Gender.dart';
import 'package:frontend/Models/User.dart';
import 'package:frontend/State/ChooseImpulseState.dart';
import 'package:mockito/mockito.dart';
import '../mocks.mocks.dart';

void main() {
  late MockUserApiHandler userApiHandler;
  late MockPaymentApiHandler paymentApiHandler;
  late MockPaymentService paymentService;
  late ChooseImpulseState state;

  setUp(() {
    userApiHandler = MockUserApiHandler();
    paymentApiHandler = MockPaymentApiHandler();
    paymentService = MockPaymentService();
    
    // Usando os mocks para injeção de dependências
    state = ChooseImpulseState()
      ..apiHandler = userApiHandler
      ..paymentApiHandler = paymentApiHandler
      ..paymentService = paymentService;
  });

  // Função para criar um usuário de teste
  User createTestUser() {
    return User(
      userId: 1,
      password: 'password123',
      firstName: 'John',
      lastName: 'Doe',
      email: 'john.doe@example.com',
      cellPhoneNumber: 123456789,
      registrationDate: DateTime.now(),
      birthDate: DateTime(1990, 1, 1),
      gender: Gender.MASCULINO,
      image: 'https://example.com/image.jpg',
      IBAN: 'GB33BUKB20201555555555',
    );
  }

  test('Estado inicial', () {
    expect(state.isLoading, false);
    expect(state.errorMessage, isNull);
  });

  test('Impulso falha se usuário não estiver autenticado', () async {
    // Simulando usuário nulo
    when(userApiHandler.getStoredUser()).thenAnswer((_) async => null);

    await state.impulse(7, 10.0, 123);

    expect(state.errorMessage, 'Erro ao criar conta');
    expect(state.isLoading, false);
  });

  test('Erro ao criar sessão de checkout', () async {
    // Criando usuário de teste
    when(userApiHandler.getStoredUser()).thenAnswer((_) async => createTestUser());
    
    // Simulando falha na criação da sessão de checkout
    when(paymentApiHandler.createImpulseCheckoutSession(any)).thenAnswer((_) async => null);

    await state.impulse(7, 10.0, 123);

    expect(state.errorMessage, 'Erro ao criar checkout session');
    expect(state.isLoading, false);
  });

  test('Erro ao abrir a URL de checkout', () async {
    when(userApiHandler.getStoredUser()).thenAnswer((_) async => createTestUser());
    when(paymentApiHandler.createImpulseCheckoutSession(any)).thenAnswer((_) async => 'checkout_id');

    // Simulando falha ao abrir a URL de checkout
    state.canLaunch = (_) async => false;

    await state.impulse(7, 10.0, 123);

    expect(state.errorMessage, 'Erro ao abrir checkout');
    expect(state.isLoading, false);
  });

  test('Impulso bem-sucedido', () async {
    when(userApiHandler.getStoredUser()).thenAnswer((_) async => createTestUser());
    when(paymentApiHandler.createImpulseCheckoutSession(any)).thenAnswer((_) async => 'checkout_id');

    // Simulando sucesso ao abrir a URL de checkout
    state.canLaunch = (_) async => true;
    state.launch = (_) async {};

    await state.impulse(7, 10.0, 123);

    expect(state.errorMessage, isNull);
    expect(state.isLoading, false);
    // Verificando se o impulso foi salvo no serviço
    verify(paymentService.saveImpulse(any)).called(1);
  });
}