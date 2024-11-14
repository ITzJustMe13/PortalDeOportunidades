import 'package:flutter/material.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Models/OpportunityImg.dart';

class CreateOpportunityScreen extends StatefulWidget {
  @override
  _AddOpportunityScreenState createState() => _AddOpportunityScreenState();
}

class _AddOpportunityScreenState extends State<CreateOpportunityScreen> {
  final _formKey = GlobalKey<FormState>();
  String _name = '';
  double _price = 0;
  int _vacancies = 0;
  OppCategory _oppcategory = OppCategory.AGRICULTURA;
  String _description = '';
  Location _location = Location.PORTO;
  String _address = '';
  DateTime _date = DateTime.now();

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Criar Oportunidade'),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Row para a imagem e os campos Nome, Descrição, Preço e Vagas
              Row(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  // Caixa de Imagem
                  Container(
                    width: 150,
                    height: 200,
                    color: Colors.grey[300], // Cor de fundo da imagem
                    child: Center(
                      child: ElevatedButton(
                        onPressed: () {},
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.green,
                        ),
                        child: const Text(
                          'Escolher Fotos',
                          style: TextStyle(color: Colors.white),
                        ),
                      ),
                    ),
                  ),
                  const SizedBox(width: 16),

                  // Coluna para os campos Nome e Descrição
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      // Nome da Oportunidade
                      SizedBox(
                        width: 415, // Ajuste para garantir que o campo tenha largura
                        child: TextFormField(
                          decoration: const InputDecoration(
                            labelText: 'Nome da Oportunidade:',
                            border: OutlineInputBorder(),
                          ),
                          validator: (value) {
                            if (value == null || value.isEmpty) {
                              return 'Por favor, insira o nome da oportunidade';
                            }
                            return null;
                          },
                          onSaved: (value) {
                            _name = value!;
                          },
                        ),
                      ),
                      const SizedBox(height: 16),

                      // Descrição da Oportunidade
                      SizedBox(
                        width: 415, // Ajuste para garantir que o campo tenha largura
                        child: TextFormField(
                          decoration: const InputDecoration(
                            labelText: 'Descrição:',
                            border: OutlineInputBorder(),
                          ),
                          validator: (value) {
                            if (value == null || value.isEmpty) {
                              return 'Por favor, insira uma descrição da oportunidade';
                            }
                            return null;
                          },
                          onSaved: (value) {
                            _description = value!;
                          },
                        ),
                      ),
                      const SizedBox(height: 16),
                      Row(
                children: [
                  // Preço
                  SizedBox(
                    width: 200, // Ajuste para garantir que o campo tenha largura
                    child: TextFormField(
                      decoration: const InputDecoration(
                        labelText: 'Preço:',
                        border: OutlineInputBorder(),
                      ),
                      keyboardType: TextInputType.numberWithOptions(decimal: true),
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Por favor, insira o preço da oportunidade';
                        }
                        return null;
                      },
                      onSaved: (value) {
                        _price = double.parse(value!);
                      },
                    ),
                  ),
                  const SizedBox(width: 16),

                  // Vagas
                  SizedBox(
                    width: 200, // Ajuste para garantir que o campo tenha largura
                    child: TextFormField(
                      decoration: const InputDecoration(
                        labelText: 'Vagas:',
                        border: OutlineInputBorder(),
                      ),
                      keyboardType: TextInputType.number,
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Por favor, insira a quantidade de vagas da oportunidade';
                        }
                        return null;
                      },
                      onSaved: (value) {
                        _vacancies = int.parse(value!);
                      },
                    ),
                  ),
                ],
              ),
                    ],
                    
                  ),
                ],
              ),

              const SizedBox(height: 16),

              // Dropdown para Categoria
              DropdownButtonFormField<String>(
                decoration: const InputDecoration(
                  labelText: 'Categoria:',
                  border: OutlineInputBorder(),
                ),
                items: ['Vindima', 'Artesanato', 'Turismo Rural', "Gastronomia"]
                    .map((category) => DropdownMenuItem<String>(
                          value: category,
                          child: Text(category),
                        ))
                    .toList(),
                onChanged: (value) {
                  setState(() {
                    OppCategory.values.forEach((category) {
                      if (category.name.toLowerCase() == value?.toLowerCase()) {
                        _oppcategory = category;
                      }
                    });
                  });
                },
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Por favor, selecione uma categoria';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // Campo para Data
              TextFormField(
                decoration: const InputDecoration(
                  labelText: 'Data:',
                  border: OutlineInputBorder(),
                ),
                readOnly: true,
                onTap: () async {
                  FocusScope.of(context).requestFocus(FocusNode());

                  DateTime? pickedDate = await showDatePicker(
                    context: context,
                    initialDate: _date,
                    firstDate: DateTime(2000),
                    lastDate: DateTime(2101),
                  );

                  if (pickedDate != null) {
                    setState(() {
                      _date = pickedDate;
                    });
                  }
                },
                controller: TextEditingController(
                  text: '${_date.toLocal()}'.split(' ')[0],
                ),
              ),
              const SizedBox(height: 16),

              // Dropdown para Localização
              DropdownButtonFormField<String>(
                decoration: const InputDecoration(
                  labelText: 'Localização:',
                  border: OutlineInputBorder(),
                ),
                items: ['Porto', 'Portalegre', 'Lisboa', "Guimaraes"]
                    .map((location) => DropdownMenuItem<String>(
                          value: location,
                          child: Text(location),
                        ))
                    .toList(),
                onChanged: (value) {
                  setState(() {
                    Location.values.forEach((location) {
                      if (location.name.toLowerCase() == value?.toLowerCase()) {
                        _location = location;
                      }
                    });
                  });
                },
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Por favor, selecione uma localização';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),

              // Campo para Endereço
              TextFormField(
                decoration: const InputDecoration(
                  labelText: 'Endereço:',
                  border: OutlineInputBorder(),
                ),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Por favor, insira o endereço da oportunidade';
                  }
                  return null;
                },
                onSaved: (value) {
                  _address = value!;
                },
              ),
            ],
          ),
        ),
      ),
    );
  }
}




