import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:frontend/Components/DynamicActionButton.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/State/SearchState.dart';
import 'package:provider/provider.dart';

class SearchDrawer extends StatelessWidget {
  final double width;
  final bool isMobile;
  const SearchDrawer({super.key, required this.width, required this.isMobile});

  @override
  Widget build(BuildContext context) {
    return Consumer<SearchState>(
      builder: (context, searchState, child) {
        return Container(
          color: isMobile ? Color(0xFFD9D9D9) : null,
          width: width,
          child: SingleChildScrollView(
            // Make the drawer scrollable if its height exceeds screen
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    "Procurar & Filtrar",
                    style: TextStyle(fontSize: 20),
                  ),
                  Divider(),
                  TextField(
                    controller: searchState.keywordController,
                    decoration: InputDecoration(
                      labelText: 'Palavra-Chave',
                      border: OutlineInputBorder(),
                    ),
                    onChanged: (text) {
                      searchState.updateKeyword(text);
                    },
                  ),
                  SizedBox(height: 8),
                  TextField(
                    controller: searchState.vacanciesController,
                    keyboardType: TextInputType.number,
                    inputFormatters: [
                      FilteringTextInputFormatter.digitsOnly,
                    ],
                    decoration: InputDecoration(
                      labelText: 'Vagas',
                      border: OutlineInputBorder(),
                    ),
                    onChanged: (text) {
                      searchState.updateVacancies(int.parse(text));
                    },
                  ),
                  SizedBox(height: 8),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Flexible(
                        child: TextField(
                          controller: searchState.minPriceController,
                          keyboardType: TextInputType.number,
                          inputFormatters: [
                            FilteringTextInputFormatter.digitsOnly,
                          ],
                          decoration: InputDecoration(
                            labelText: 'Preço Min',
                            border: OutlineInputBorder(),
                          ),
                          onChanged: (text) {
                            searchState.updateMinPrice(double.parse(text));
                          },
                        ),
                      ),
                      const SizedBox(width: 8),
                      Flexible(
                        child: TextField(
                          controller:searchState.maxPriceController,
                          keyboardType: TextInputType.number,
                          inputFormatters: [
                            FilteringTextInputFormatter.digitsOnly,
                          ],
                          decoration: InputDecoration(
                            labelText: 'Preço Max',
                            border: OutlineInputBorder(),
                          ),
                          onChanged: (text) {
                            searchState.updateMaxPrice(double.parse(text));
                          },
                        ),
                      ),
                    ],
                  ),
                  SizedBox(height: 8),
                  DropdownButton<OppCategory>(
                    value: searchState.category,
                    hint: Text("Selecionar Categoria"),
                    onChanged: (OppCategory? newValue) {
                      searchState.updateCategory(newValue);
                    },
                    isExpanded:
                        true, // Make the dropdown button take full width
                    items: OppCategory.values.map((OppCategory category) {
                      return DropdownMenuItem<OppCategory>(
                        value: category,
                        child: Text(
                          category.name.replaceAll(
                              '_', ' '), // Replace underscores with spaces
                          overflow: TextOverflow
                              .ellipsis, // Truncate with ellipsis if too long
                          maxLines: 2, // Ensure only one line of text
                        ),
                      );
                    }).toList(),
                    selectedItemBuilder: (BuildContext context) {
                      return OppCategory.values.map((OppCategory category) {
                        return Text(
                          category.name.replaceAll('_', ' '),
                          overflow: TextOverflow
                              .ellipsis, // Truncate with ellipsis for the selected item
                          maxLines:
                              1, // Ensure only one line of text for the selected value
                        );
                      }).toList();
                    },
                  ),
                  SizedBox(height: 8),
                  DropdownButton<Location>(
                    value:searchState.location,
                    hint: Text("Selecionar Localização"),
                    onChanged: (Location? newValue) {
                      searchState.updateLocation(newValue);
                    },
                    isExpanded:
                        true, // Make the dropdown button take full width
                    items: Location.values.map((Location location) {
                      return DropdownMenuItem<Location>(
                        value: location,
                        child: Text(
                          location.name.replaceAll(
                              '_', ' '), // Replace underscores with spaces
                          overflow: TextOverflow
                              .ellipsis, // Truncate with ellipsis if too long
                          maxLines: 2, // Ensure only one line of text
                        ),
                      );
                    }).toList(),
                    selectedItemBuilder: (BuildContext context) {
                      return Location.values.map((Location location) {
                        return Text(
                          location.name.replaceAll('_', ' '),
                          overflow: TextOverflow
                              .ellipsis, // Truncate with ellipsis for the selected item
                          maxLines:
                              1, // Ensure only one line of text for the selected value
                        );
                      }).toList();
                    },
                  ),
                  SizedBox(height: 8),
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    crossAxisAlignment: CrossAxisAlignment.center,
                    children: [
                      Center(
                        child: DynamicActionButton(
                            onPressed: () async {
                              await searchState.search();
                            },
                            text: "Filtrar",
                            icon: Icons.filter,
                            color: Color(0xFF50C878)),
                      ),
                      SizedBox(width: 8),
                      Center(
                          child: IconButton(
                              onPressed: () => searchState.clear(),
                              icon: Icon(Icons.clear))),
                    ],
                  ),
                ],
              ),
            ),
          ),
        );
      },
    );
  }
}
