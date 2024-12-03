// Mock da classe OpportunityApiHandler
import 'package:flutter_test/flutter_test.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Models/OpportunityImg.dart';
import 'package:frontend/State/SearchState.dart';
import 'package:mockito/mockito.dart';
import '../mocks.mocks.dart';

void main() {
  late MockOpportunityApiHandler mockOpportunityApiHandler;
  late SearchState searchState;

  setUp(() {
    mockOpportunityApiHandler = MockOpportunityApiHandler();
    searchState = SearchState();
    searchState.opportunityApiHandler = mockOpportunityApiHandler; // Configura a dependência
  });

  test('Deve inicializar a lista de oportunidades', () async {
    // Criando uma imagem de oportunidade
    var opportunityImg = OpportunityImg(
      imgId: 1,
      opportunityId: 1,
      imageBase64: 'image_base64_string',
    );

    // Criando uma oportunidade com imagens
    var opportunity = Opportunity(
      opportunityId: 1,
      name: 'Oportunidade Teste',
      price: 100.0,
      vacancies: 3,
      isActive: true,
      category: OppCategory.AGRICULTURA,
      description: 'Descrição da oportunidade',
      location: Location.ACORES,
      address: 'Endereço Teste',
      userId: 1,
      reviewScore: 4.5,
      date: DateTime.now(),
      isImpulsed: false,
      opportunityImgs: [opportunityImg],
    );

    // Simulando a resposta da API
    when(mockOpportunityApiHandler.getAllOpportunities())
        .thenAnswer((_) async => [opportunity]);

    await searchState.initializeOpportunitiesList();

    // Verificando se a lista de oportunidades foi preenchida corretamente
    expect(searchState.opportunitiesList?.length, 1);
    expect(searchState.isLoading, false);
    expect(searchState.opportunitiesList?.first.name, 'Oportunidade Teste');
  });

  test('Deve realizar a busca de oportunidades com filtros', () async {
    var opportunityImg = OpportunityImg(
      imgId: 1,
      opportunityId: 2,
      imageBase64: 'image_base64_string',
    );

    var opportunity = Opportunity(
      opportunityId: 2,
      name: 'Oportunidade Teste 2',
      price: 150.0,
      vacancies: 5,
      isActive: true,
      category: OppCategory.AGRICULTURA,
      description: 'Descrição da oportunidade 2',
      location: Location.ACORES,
      address: 'Endereço Teste 2',
      userId: 2,
      reviewScore: 4.8,
      date: DateTime.now(),
      isImpulsed: true,
      opportunityImgs: [opportunityImg],
    );

    // Simulando a resposta da API para busca
    when(mockOpportunityApiHandler.searchOpportunities(
      'keyword', 10, 100.0, 200.0, null, null,
    )).thenAnswer((_) async => [opportunity]);

    searchState.updateKeyword('keyword');
    searchState.updateVacancies(10);
    searchState.updateMinPrice(100.0);
    searchState.updateMaxPrice(200.0);

    await searchState.search();

    // Verificando se a busca foi realizada com os parâmetros corretos
    expect(searchState.opportunitiesList?.length, 1);
    expect(searchState.isLoading, false);
  });

  test('Deve realizar a ordenação por preço', () async {
    var opportunityImg1 = OpportunityImg(
      imgId: 1,
      opportunityId: 1,
      imageBase64: 'image_base64_string_1',
    );

    var opportunityImg2 = OpportunityImg(
      imgId: 2,
      opportunityId: 2,
      imageBase64: 'image_base64_string_2',
    );

    var opportunity1 = Opportunity(
      opportunityId: 1,
      name: 'Oportunidade 1',
      price: 200.0,
      vacancies: 5,
      isActive: true,
      category: OppCategory.AGRICULTURA,
      description: 'Descrição 1',
      location: Location.ACORES,
      address: 'Endereço 1',
      userId: 1,
      reviewScore: 4.5,
      date: DateTime.now(),
      isImpulsed: false,
      opportunityImgs: [opportunityImg1],
    );

    var opportunity2 = Opportunity(
      opportunityId: 2,
      name: 'Oportunidade 2',
      price: 100.0,
      vacancies: 3,
      isActive: true,
      category: OppCategory.AGRICULTURA,
      description: 'Descrição 2',
      location: Location.ACORES,
      address: 'Endereço 2',
      userId: 2,
      reviewScore: 4.8,
      date: DateTime.now(),
      isImpulsed: true,
      opportunityImgs: [opportunityImg2],
    );

    // Simulando a resposta da API para busca
    when(mockOpportunityApiHandler.getAllOpportunities())
        .thenAnswer((_) async => [opportunity1, opportunity2]);

    await searchState.initializeOpportunitiesList();

    // Ordenando por preço
    await searchState.sortByPrice();

    // Verificando se as oportunidades foram ordenadas corretamente por preço
    expect(searchState.opportunitiesList?.first.price, 200.0);
    expect(searchState.isLoading, false);
  });
}