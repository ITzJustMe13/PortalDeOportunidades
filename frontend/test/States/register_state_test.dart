import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/Enums/Gender.dart';
import 'package:frontend/State/RegisterState.dart';
import 'package:frontend/Models/User.dart';
import 'package:mockito/mockito.dart';
import '../mocks.mocks.dart';

void main() {
  late RegisterState registerState;
  late MockUserApiHandler mockUserApiHandler;

  setUp(() {
    mockUserApiHandler = MockUserApiHandler();
    registerState = RegisterState();
    registerState.userApiHandler = mockUserApiHandler; // Usar o setter para injetar o mock
  });

  test('Deve registrar um usuário com sucesso', () async {
    final user = User(
      userId: 1,
      firstName: 'John',
      lastName: 'Doe',
      password: 'password123',
      birthDate: DateTime(1990, 1, 1),
      registrationDate: DateTime.now(),
      email: 'john.doe@example.com',
      cellPhoneNumber: 123456789,
      gender: Gender.MASCULINO,
      image: 'image_path',
      IBAN: 'IBAN12345',
    );

    // Configura o mock para retornar o usuário corretamente
    when(mockUserApiHandler.createUser(any)).thenAnswer((_) async => user);

    // Executa o registro
    await registerState.register(user, MockBuildContext());

    // Verifica se o processo de registro foi bem-sucedido
    expect(registerState.isLoading, false);
    expect(registerState.isActivationSuccess, true);
    expect(registerState.errorMessage, isNull);
  });

  test('Deve falhar ao registrar um usuário com dados inválidos', () async {
    final user = User(
      userId: 1,
      firstName: '',
      lastName: '',
      password: '',
      birthDate: DateTime(1990, 1, 1),
      registrationDate: DateTime.now(),
      email: '',
      cellPhoneNumber: 0,
      gender: Gender.MASCULINO,
      image: '',
      IBAN: '',
    );

    // Configura o mock para retornar null (falha ao criar o usuário)
    when(mockUserApiHandler.createUser(any)).thenAnswer((_) async => null);

    // Executa o registro com dados inválidos
    await registerState.register(user, MockBuildContext());

    // Verifica se o erro foi tratado corretamente
    expect(registerState.isLoading, false);
    expect(registerState.isActivationSuccess, false);
    expect(registerState.errorMessage, 'Erro ao criar conta');
  });
}