import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/State/DrawerState.dart';
import 'package:frontend/State/LoginState.dart';
import 'package:mockito/mockito.dart';

class MockLoginState extends Mock implements LoginState {}

class MockNavigatorObserver extends Mock implements NavigatorObserver {}

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();
  
  late MockLoginState mockLoginState;
  late CustomDrawerState customDrawerState;
  late MockNavigatorObserver mockObserver;

  setUp(() {
    mockLoginState = MockLoginState();
    mockObserver = MockNavigatorObserver();

    customDrawerState = CustomDrawerState(
      loginState: mockLoginState,
    );
  });

  test('Testar navegação quando o usuário está logado', () {
    // Simula o login
    when(mockLoginState.isLoggedIn).thenReturn(true);

    final route = '/login';

    // Simula a navegação
    customDrawerState.ensureUserLoggedIn(route);

    // Verifica que a navegação foi chamada
    verify(mockObserver.didPush(any, any)).called(1);
    verifyNoMoreInteractions(mockObserver);
  });

  test('Testar exibição do login dialog quando o usuário não está logado', () {
    // Simula que o usuário não está logado
    when(mockLoginState.isLoggedIn).thenReturn(false);

    final route = '/';

    // Simula a navegação, o dialog deve ser mostrado
    customDrawerState.ensureUserLoggedIn(route);

    // Verifica que o dialog de login foi mostrado (simulação através do print)
    // Verifique se o método '_showLoginDialog' foi chamado.
    // Aqui você pode usar o método `verify` do Mockito para verificar.
    verify(mockLoginState.isLoggedIn).called(1);
    // Verificando se a navegação não foi chamada
    expect(routeCalled, '');
  });

  test('Testar logout e navegação para a rota de login', () async {
    // Simula o login
    when(mockLoginState.isLoggedIn).thenReturn(true);

    final route = '/login';

    // Simula o logout
    customDrawerState.logout(route);

    // Verifica que o usuário foi desconectado
    verify(mockLoginState.logout()).called(1);
    // Verifica que a navegação foi chamada para a rota de login
    verify(mockObserver.didPush(any, any)).called(1);
  });
}