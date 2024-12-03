import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/Enums/Gender.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/OpportunityImg.dart';
import 'package:frontend/Models/User.dart';
import 'package:frontend/Services/payment_api_handler.dart';
import 'package:frontend/Services/payment_service.dart';
import 'package:frontend/Services/user_api_handler.dart';
import 'package:frontend/State/ChooseImpulseState.dart';
import 'package:frontend/State/CreateOpportunityState.dart';
import 'package:mockito/mockito.dart';

import '../mocks.mocks.dart';

void main() {
  late CreateOpportunityState createOpportunityState;
  late MockUserApiHandler mockUserApiHandler;
  late MockOpportunityApiHandler mockOpportunityApiHandler;

  setUp(() {
    mockUserApiHandler = MockUserApiHandler();
    mockOpportunityApiHandler = MockOpportunityApiHandler();

    createOpportunityState = CreateOpportunityState()
      ..userApiHandler = mockUserApiHandler
      ..opportunityApiHandler = mockOpportunityApiHandler;
  });

  test('Deve retornar o ID do usuário atual', () async {
    // Configurando o mock para retornar um ID de usuário
    when(mockUserApiHandler.getStoredUserID()).thenAnswer((_) async => 123);

    // Chamando o método getCurrentUserId
    final userId = await createOpportunityState.getCurrentUserId();

    // Verificando se o retorno está correto
    expect(userId, 123);
    verify(mockUserApiHandler.getStoredUserID()).called(1);
  });

  test('Deve criar uma oportunidade com sucesso', () async {
    final opportunity = Opportunity(
      opportunityId: 1,
      name: 'Nova Oportunidade',
      price: 100.0,
      vacancies: 5,
      isActive: true,
      category: OppCategory.AGRICULTURA,
      description: 'Descrição da oportunidade',
      location: Location.ACORES,
      address: 'Rua Exemplo, 123',
      userId: 123,
      reviewScore: 4.5,
      date: DateTime.now(),
      isImpulsed: false,
      opportunityImgs: [
        OpportunityImg(imgId: 0, opportunityId: 1, imageBase64: 'imageBase64Data')
      ],
    );

    final opportunityImgs = [
      OpportunityImg(imgId: 0, opportunityId: 1, imageBase64: 'imageBase64Data')
    ];

    // Configurando os mocks para retornar sucesso na criação e edição da oportunidade
    when(mockOpportunityApiHandler.createOpportunity(any))
        .thenAnswer((_) async => opportunity);
    when(mockOpportunityApiHandler.editOpportunity(any, any)).thenAnswer((_) async => true);

    // Chamando o método createOpportunity
    final createdOpportunity = await createOpportunityState.createOpportunity(opportunity, opportunityImgs);

    // Verificando se a oportunidade foi criada com sucesso
    expect(createdOpportunity, isNotNull);
    expect(createdOpportunity?.opportunityId, 1);
    expect(createOpportunityState.isLoading, false);
    expect(createOpportunityState.errorMessage, isNull);

    verify(mockOpportunityApiHandler.createOpportunity(any)).called(1);
    verify(mockOpportunityApiHandler.editOpportunity(any, any)).called(1);
  });

  test('Deve falhar ao criar uma oportunidade e retornar erro', () async {
    final opportunity = Opportunity(
      opportunityId: 1,
      name: 'Nova Oportunidade',
      price: 100.0,
      vacancies: 5,
      isActive: true,
      category: OppCategory.AGRICULTURA,
      description: 'Descrição da oportunidade',
      location: Location.ACORES,
      address: 'Rua Exemplo, 123',
      userId: 123,
      reviewScore: 4.5,
      date: DateTime.now(),
      isImpulsed: false,
      opportunityImgs: [
        OpportunityImg(imgId: 0, opportunityId: 1, imageBase64: 'imageBase64Data')
      ],
    );

    final opportunityImgs = [
      OpportunityImg(imgId: 0, opportunityId: 1, imageBase64: 'imageBase64Data')
    ];

    // Configurando o mock para falhar na criação da oportunidade
    when(mockOpportunityApiHandler.createOpportunity(any))
        .thenAnswer((_) async => null);

    // Chamando o método createOpportunity
    final createdOpportunity = await createOpportunityState.createOpportunity(opportunity, opportunityImgs);

    // Verificando se a oportunidade não foi criada
    expect(createdOpportunity, isNull);
    expect(createOpportunityState.isLoading, false);
    expect(createOpportunityState.errorMessage, 'Ocorreu um erro ao criar Oportunidade');

    verify(mockOpportunityApiHandler.createOpportunity(any)).called(1);
  });

  test('Deve falhar ao editar a oportunidade e retornar erro', () async {
    final opportunity = Opportunity(
      opportunityId: 1,
      name: 'Nova Oportunidade',
      price: 100.0,
      vacancies: 5,
      isActive: true,
      category: OppCategory.AGRICULTURA,
      description: 'Descrição da oportunidade',
      location: Location.ACORES,
      address: 'Rua Exemplo, 123',
      userId: 123,
      reviewScore: 4.5,
      date: DateTime.now(),
      isImpulsed: false,
      opportunityImgs: [
        OpportunityImg(imgId: 0, opportunityId: 1, imageBase64: 'imageBase64Data')
      ],
    );

    final opportunityImgs = [
      OpportunityImg(imgId: 0, opportunityId: 1, imageBase64: 'imageBase64Data')
    ];

    // Configurando o mock para retornar uma oportunidade válida
    when(mockOpportunityApiHandler.createOpportunity(any))
        .thenAnswer((_) async => opportunity);

    // Configurando o mock para falhar na edição da oportunidade
    when(mockOpportunityApiHandler.editOpportunity(any, any)).thenAnswer((_) async => false);

    // Chamando o método createOpportunity
    final createdOpportunity = await createOpportunityState.createOpportunity(opportunity, opportunityImgs);

    // Verificando se a oportunidade não foi criada
    expect(createdOpportunity, isNull);
    expect(createOpportunityState.isLoading, false);
    expect(createOpportunityState.errorMessage, 'Ocorreu um erro ao criar Oportunidade');

    verify(mockOpportunityApiHandler.createOpportunity(any)).called(1);
    verify(mockOpportunityApiHandler.editOpportunity(any, any)).called(1);
  });
}